import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe, registerLocaleData } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { WeddingService } from '../../services/wedding';
import { take } from 'rxjs';
import localePt from '@angular/common/locales/pt';
import { ListType, LIST_TYPE_LABELS, IWeddingListShort } from '../../interfaces/wedding';

// Register Portuguese locale
registerLocaleData(localePt);

@Component({
  selector: 'app-create-wedding-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  providers: [DatePipe],
  templateUrl: './create-wedding-list.html',
  styleUrl: './create-wedding-list.css'
})
export class CreateWeddingList implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _weddingService = inject(WeddingService);
  private readonly _router = inject(Router);
  private readonly _datePipe = inject(DatePipe);

  isSubmitting = false;
  errorMessage = '';
  
  // Track existing list types
  existingListTypes: Set<ListType> = new Set();

  // List type options
  ListType = ListType;
  LIST_TYPE_LABELS = LIST_TYPE_LABELS;

  form = this._fb.group({
    listType: [ListType.Wedding as ListType, [Validators.required]],
    title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    message: ['', [Validators.maxLength(500)]],
    eventDate: ['', [Validators.required]],
    deliveryInfo: ['', [Validators.maxLength(500)]]
  });

  // Step control for wizard-like experience
  currentStep = 1;
  totalSteps = 4;

  ngOnInit() {
    this.loadExistingLists();
  }

  loadExistingLists() {
    this._weddingService.getMyWeddingLists().pipe(take(1)).subscribe({
      next: (response) => {
        response.lists.forEach(list => {
          this.existingListTypes.add(list.listType);
        });
        // Se o tipo padrão (Wedding) já existe, selecione o outro tipo disponível
        if (this.existingListTypes.has(ListType.Wedding) && !this.existingListTypes.has(ListType.BridalShower)) {
          this.form.get('listType')?.setValue(ListType.BridalShower);
        }
      }
    });
  }

  nextStep() {
    if (this.currentStep < this.totalSteps) {
      // Validate current step before proceeding
      if (this.currentStep === 1) {
        if (this.form.get('listType')?.invalid) {
          this.form.get('listType')?.markAsTouched();
          return;
        }
      }
      if (this.currentStep === 2) {
        if (this.form.get('title')?.invalid) {
          this.form.get('title')?.markAsTouched();
          return;
        }
      }
      if (this.currentStep === 3) {
        if (this.form.get('eventDate')?.invalid) {
          this.form.get('eventDate')?.markAsTouched();
          return;
        }
      }
      this.currentStep++;
    }
  }

  prevStep() {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  goToStep(step: number) {
    if (step <= this.currentStep || step === this.currentStep + 1) {
      this.currentStep = step;
    }
  }

  selectListType(type: ListType) {
    if (!this.existingListTypes.has(type)) {
      this.form.get('listType')?.setValue(type);
    }
  }

  isTypeDisabled(type: ListType): boolean {
    return this.existingListTypes.has(type);
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const formValue = this.form.value;
    
    this._weddingService.createWeddingList({
      title: formValue.title!,
      message: formValue.message || undefined,
      eventDate: formValue.eventDate!,
      deliveryInfo: formValue.deliveryInfo || undefined,
      listType: formValue.listType!
    }).pipe(take(1)).subscribe({
      next: (response) => {
        this.isSubmitting = false;
        // Navigate to the manage list page
        this._router.navigate(['/gerenciar-lista', response.id]);
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.errors?.[0] || error.error?.message || 'Erro ao criar evento. Tente novamente.';
      }
    });
  }

  // Helper to get minimum date (today)
  get minDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  // Format date string (YYYY-MM-DD) to readable format
  formatEventDate(dateString: string | null | undefined): string {
    if (!dateString) return '';
    // Parse the date string and create a Date object
    const [year, month, day] = dateString.split('-').map(Number);
    const date = new Date(year, month - 1, day);
    return this._datePipe.transform(date, 'fullDate', '', 'pt-BR') || '';
  }
}
