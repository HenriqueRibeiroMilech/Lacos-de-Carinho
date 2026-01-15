import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-pagamento-pendente',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './pagamento-pendente.html',
  styleUrl: './pagamento-pendente.css'
})
export class PagamentoPendente {

}
