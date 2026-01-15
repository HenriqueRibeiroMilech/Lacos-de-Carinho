import { Routes } from '@angular/router';

// Pages
import { Landing } from './pages/landing/landing';
import { Login } from './pages/login/login';
import { Cadastro } from './pages/cadastro/cadastro';
import { Register } from './pages/register/register';
import { ForgotPassword } from './pages/forgot-password/forgot-password';
import { ResetPassword } from './pages/reset-password/reset-password';
import { Dashboard } from './pages/dashboard/dashboard';
import { CreateWeddingList } from './pages/create-wedding-list/create-wedding-list';
import { ManageWeddingList } from './pages/manage-wedding-list/manage-wedding-list';
import { PublicList } from './pages/public-list/public-list';
import { PagamentoSucesso } from './pages/pagamento-sucesso/pagamento-sucesso';
import { PagamentoFalha } from './pages/pagamento-falha/pagamento-falha';
import { PagamentoPendente } from './pages/pagamento-pendente/pagamento-pendente';

// Guards
import { authGuard } from './guards/auth-guard';
import { loginAuthGuard } from './guards/login-auth-guard';

export const routes: Routes = [
  // ============================================
  // PÁGINA DE LOGIN (DEFAULT)
  // ============================================
  {
    path: '',
    redirectTo: 'entrar',
    pathMatch: 'full',
  },
  {
    path: 'entrar',
    component: Login,
    canActivate: [loginAuthGuard],
  },

  // ============================================
  // LANDING PAGE (para anúncios/marketing)
  // ============================================
  {
    path: 'inicio',
    component: Landing,
  },

  // ============================================
  // ROTAS DE AUTENTICAÇÃO
  // ============================================
  
  // Cadastro para CASAIS (vem da Landing) - role=admin
  {
    path: 'cadastro',
    component: Cadastro,
    canActivate: [loginAuthGuard],
  },
  
  // Registro para CONVIDADOS (vem da lista pública) - role=user
  {
    path: 'registro',
    component: Register,
    canActivate: [loginAuthGuard],
  },
  
  {
    path: 'esqueci-senha',
    component: ForgotPassword,
    canActivate: [loginAuthGuard],
  },
  {
    path: 'redefinir-senha',
    component: ResetPassword,
    canActivate: [loginAuthGuard],
  },

  // ============================================
  // ROTAS PROTEGIDAS
  // ============================================
  {
    path: 'painel',
    component: Dashboard,
    canActivate: [authGuard],
  },
  {
    path: 'criar-evento',
    component: CreateWeddingList,
    canActivate: [authGuard],
  },
  {
    path: 'gerenciar-lista/:id',
    component: ManageWeddingList,
    canActivate: [authGuard],
  },

  // ============================================
  // ROTAS PÚBLICAS
  // ============================================
  {
    path: 'lista/:link',
    component: PublicList,
  },

  // ============================================
  // ROTAS DE PAGAMENTO
  // ============================================
  {
    path: 'pagamento-sucesso',
    component: PagamentoSucesso,
  },
  {
    path: 'pagamento-falha',
    component: PagamentoFalha,
  },
  {
    path: 'pagamento-pendente',
    component: PagamentoPendente,
  },

  // ============================================
  // REDIRECTS (compatibilidade com rotas antigas)
  // ============================================
  { path: 'login', redirectTo: 'entrar', pathMatch: 'full' },
  { path: 'register', redirectTo: 'registro', pathMatch: 'full' },
  { path: 'forgot-password', redirectTo: 'esqueci-senha', pathMatch: 'full' },
  { path: 'reset-password', redirectTo: 'redefinir-senha', pathMatch: 'full' },
  { path: 'dashboard', redirectTo: 'painel', pathMatch: 'full' },
  { path: 'create-event', redirectTo: 'criar-evento', pathMatch: 'full' },
  
  // Fallback
  {
    path: '**',
    redirectTo: 'entrar',
  }
];
