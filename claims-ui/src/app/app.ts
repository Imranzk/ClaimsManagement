
import { Component, OnInit, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterOutlet } from '@angular/router';
import { FormControl, FormsModule, Validators } from '@angular/forms';
import { ClaimService, DashboardData, Claim } from './claim.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CurrencyPipe, DatePipe, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  today = (() => {
    const date = new Date();
    date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
    return date.toISOString().slice(0, 10);
  })();

claimForm = {
  claimantName: '',
  claimantEmail: '',
  market: 'Malaysia',
  claimType: 0,
  incidentDate: '',
  incidentDescription: '',
  estimatedAmount: null as number | null
};

isSubmitting = signal(false);
submitMessage = signal('');
submitError = signal('');
  activePage = signal('Dashboard');

  dashboard = signal<DashboardData | null>(null);
  claims = signal<Claim[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');
  allClaims = signal<Claim[]>([]);
  isLoadingAllClaims = signal(false);
  allClaimsError = signal('');
  assignmentOfficerNames = signal<Record<string, string>>({});
  assignmentMessages = signal<Record<string, string>>({});
  assignmentErrors = signal<Record<string, string>>({});
  assigningClaimId = signal<string | null>(null);
  informationRequests = signal<Record<string, string>>({});
  informationRequestMessages = signal<Record<string, string>>({});
  informationRequestErrors = signal<Record<string, string>>({});
  requestingInformationClaimId = signal<string | null>(null);
  decisionReasons = signal<Record<string, string>>({});
  decisionMessages = signal<Record<string, string>>({});
  decisionErrors = signal<Record<string, string>>({});
  decidingClaimId = signal<string | null>(null);
  settlementMessages = signal<Record<string, string>>({});
  settlementErrors = signal<Record<string, string>>({});
  settlingClaimId = signal<string | null>(null);
  trackingClaimNumber = signal('');
  trackedClaim = signal<Claim | null>(null);
  isTracking = signal(false);
  trackingError = signal('');
  myClaimsEmail = signal('');
  myClaims = signal<Claim[]>([]);
  hasSearchedMyClaims = signal(false);
  isSearchingMyClaims = signal(false);
  myClaimsError = signal('');
  claimantAdditionalInformation = signal<Record<string, string>>({});
  informationProvidedMessages = signal<Record<string, string>>({});
  informationProvidedErrors = signal<Record<string, string>>({});
  providingInformationClaimId = signal<string | null>(null);

  navigation = [
    { name: 'Dashboard', icon: '▦' },
    { name: 'All Claims', icon: '▤' },
    { name: 'Submit Claim', icon: '+' },
    { name: 'My Claims', icon: '◷' },
    { name: 'Track Claim', icon: '⌕' }
  ];

  constructor(private claimService: ClaimService) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    if (this.isLoading()) {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    this.claimService.getDashboard().subscribe({
      next: (data) => {
        this.dashboard.set(data);
        this.claims.set(data.recentClaims ?? []);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Failed to load dashboard:', error);
        this.errorMessage.set(
          'Unable to load dashboard data. Please check that the API is running.'
        );
        this.isLoading.set(false);
      }
    });
  }

  setPage(page: string): void {
    this.activePage.set(page);

    if (page === 'All Claims') {
      this.loadAllClaims();
    }
  }

  loadAllClaims(): void {
    if (this.isLoadingAllClaims()) {
      return;
    }

    this.isLoadingAllClaims.set(true);
    this.allClaimsError.set('');

    this.claimService.getClaims().subscribe({
      next: (claims) => {
        this.allClaims.set(claims);
        this.isLoadingAllClaims.set(false);
      },
      error: (error) => {
        console.error('Failed to load all claims:', error);
        this.allClaimsError.set(
          'Unable to load claims right now. Please try again.'
        );
        this.isLoadingAllClaims.set(false);
      }
    });
  }

  setAssignmentOfficer(claimId: string, officerName: string): void {
    this.assignmentOfficerNames.update((names) => ({
      ...names,
      [claimId]: officerName
    }));
  }

  assignClaim(claim: Claim): void {
    if (
      claim.status !== 0 ||
      this.assigningClaimId() ||
      this.requestingInformationClaimId() ||
      this.decidingClaimId() ||
      this.settlingClaimId()
    ) {
      return;
    }

    const assignedOfficer = this.assignmentOfficerNames()[claim.id]?.trim();
    this.assignmentMessages.update((messages) => ({
      ...messages,
      [claim.id]: ''
    }));
    this.assignmentErrors.update((errors) => ({
      ...errors,
      [claim.id]: ''
    }));

    if (!assignedOfficer) {
      this.assignmentErrors.update((errors) => ({
        ...errors,
        [claim.id]: 'Enter an officer name before assigning this claim.'
      }));
      return;
    }

    this.assigningClaimId.set(claim.id);

    this.claimService.assignClaim(claim.id, {
      assignedOfficer,
      performedBy: 'Demo Claims Officer'
    }).subscribe({
      next: (response) => {
        this.assignmentMessages.update((messages) => ({
          ...messages,
          [claim.id]: response.message
        }));
        this.assigningClaimId.set(null);
        this.loadAllClaims();
      },
      error: (error) => {
        console.error('Failed to assign claim:', error);
        this.assignmentErrors.update((errors) => ({
          ...errors,
          [claim.id]:
            error.error?.message ?? 'Unable to assign this claim. Please try again.'
        }));
        this.assigningClaimId.set(null);
      }
    });
  }

  setInformationRequest(claimId: string, informationRequest: string): void {
    this.informationRequests.update((requests) => ({
      ...requests,
      [claimId]: informationRequest
    }));
  }

  requestAdditionalInformation(claim: Claim): void {
    if (
      claim.status !== 1 ||
      this.assigningClaimId() ||
      this.requestingInformationClaimId() ||
      this.decidingClaimId() ||
      this.settlingClaimId()
    ) {
      return;
    }

    const informationRequest = this.informationRequests()[claim.id]?.trim();
    this.informationRequestMessages.update((messages) => ({
      ...messages,
      [claim.id]: ''
    }));
    this.informationRequestErrors.update((errors) => ({
      ...errors,
      [claim.id]: ''
    }));

    if (!informationRequest) {
      this.informationRequestErrors.update((errors) => ({
        ...errors,
        [claim.id]: 'Enter a message describing the information needed.'
      }));
      return;
    }

    this.requestingInformationClaimId.set(claim.id);

    this.claimService.requestInformation(claim.id, {
      informationRequest,
      performedBy: 'Demo Claims Officer'
    }).subscribe({
      next: (response) => {
        this.informationRequestMessages.update((messages) => ({
          ...messages,
          [claim.id]: response.message
        }));
        this.requestingInformationClaimId.set(null);
        this.loadAllClaims();
      },
      error: (error) => {
        console.error('Failed to request additional information:', error);
        this.informationRequestErrors.update((errors) => ({
          ...errors,
          [claim.id]:
            error.error?.message ??
            'Unable to request additional information. Please try again.'
        }));
        this.requestingInformationClaimId.set(null);
      }
    });
  }

  setDecisionReason(claimId: string, decisionReason: string): void {
    this.decisionReasons.update((reasons) => ({
      ...reasons,
      [claimId]: decisionReason
    }));
  }

  decideClaim(claim: Claim, decision: 'approve' | 'reject'): void {
    if (
      (claim.status !== 1 && claim.status !== 3) ||
      this.assigningClaimId() ||
      this.requestingInformationClaimId() ||
      this.decidingClaimId() ||
      this.settlingClaimId()
    ) {
      return;
    }

    const decisionReason = this.decisionReasons()[claim.id]?.trim();
    this.decisionMessages.update((messages) => ({
      ...messages,
      [claim.id]: ''
    }));
    this.decisionErrors.update((errors) => ({
      ...errors,
      [claim.id]: ''
    }));

    if (!decisionReason) {
      this.decisionErrors.update((errors) => ({
        ...errors,
        [claim.id]: 'Enter a decision reason before proceeding.'
      }));
      return;
    }

    const action = decision;
    const confirmationMessage = decision === 'reject'
      ? `Are you sure you want to reject claim ${claim.claimNumber}? This decision will be recorded.`
      : `Are you sure you want to approve claim ${claim.claimNumber}?`;

    if (!window.confirm(confirmationMessage)) {
      return;
    }

    this.decidingClaimId.set(claim.id);

    const request = {
      decisionReason,
      performedBy: 'Demo Claims Officer'
    };
    const actionRequest = decision === 'approve'
      ? this.claimService.approveClaim(claim.id, request)
      : this.claimService.rejectClaim(claim.id, request);

    actionRequest.subscribe({
      next: (response) => {
        this.decisionMessages.update((messages) => ({
          ...messages,
          [claim.id]: response.message
        }));
        this.decidingClaimId.set(null);
        this.loadAllClaims();
      },
      error: (error) => {
        console.error(`Failed to ${action} claim:`, error);
        this.decisionErrors.update((errors) => ({
          ...errors,
          [claim.id]:
            error.error?.message ??
            `Unable to ${action} this claim. Please try again.`
        }));
        this.decidingClaimId.set(null);
      }
    });
  }

  settleClaim(claim: Claim): void {
    if (
      claim.status !== 4 ||
      this.assigningClaimId() ||
      this.requestingInformationClaimId() ||
      this.decidingClaimId() ||
      this.settlingClaimId()
    ) {
      return;
    }

    this.settlementMessages.update((messages) => ({
      ...messages,
      [claim.id]: ''
    }));
    this.settlementErrors.update((errors) => ({
      ...errors,
      [claim.id]: ''
    }));

    if (!window.confirm(`Are you sure you want to settle claim ${claim.claimNumber}?`)) {
      return;
    }

    this.settlingClaimId.set(claim.id);

    this.claimService.settleClaim(claim.id, {
      performedBy: 'Demo Claims Officer'
    }).subscribe({
      next: (response) => {
        this.settlementMessages.update((messages) => ({
          ...messages,
          [claim.id]: response.message
        }));
        this.settlingClaimId.set(null);
        this.loadAllClaims();
      },
      error: (error) => {
        console.error('Failed to settle claim:', error);
        this.settlementErrors.update((errors) => ({
          ...errors,
          [claim.id]:
            error.error?.message ?? 'Unable to settle this claim. Please try again.'
        }));
        this.settlingClaimId.set(null);
      }
    });
  }

  trackClaim(): void {
    const claimNumber = this.trackingClaimNumber().trim();

    this.trackedClaim.set(null);
    this.trackingError.set('');

    if (!claimNumber) {
      this.trackingError.set('Please enter a claim number to search.');
      return;
    }

    if (this.isTracking()) {
      return;
    }

    this.isTracking.set(true);

    this.claimService.trackClaim(claimNumber).subscribe({
      next: (claim) => {
        this.trackedClaim.set(claim);
        this.isTracking.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.trackingError.set(
          error.status === 404
            ? 'We could not find a claim with that number. Please check it and try again.'
            : 'Unable to retrieve the claim right now. Please try again later.'
        );
        this.isTracking.set(false);
      }
    });
  }

  searchMyClaims(): void {
    if (this.isSearchingMyClaims() || this.providingInformationClaimId()) {
      return;
    }

    const email = this.myClaimsEmail().trim();
    this.myClaims.set([]);
    this.hasSearchedMyClaims.set(false);
    this.myClaimsError.set('');

    if (!email) {
      this.myClaimsError.set('Please enter the email address used when submitting your claim.');
      return;
    }

    if (!this.isValidEmail(email) || email.length > 256) {
      this.myClaimsError.set('Please enter a valid email address.');
      return;
    }

    this.isSearchingMyClaims.set(true);

    this.claimService.getClaims(email).subscribe({
      next: (claims) => {
        this.myClaims.set(claims);
        this.hasSearchedMyClaims.set(true);
        this.isSearchingMyClaims.set(false);
      },
      error: (error) => {
        console.error('Failed to search claims:', error);
        this.myClaimsError.set(
          'Unable to search claims right now. Please try again later.'
        );
        this.isSearchingMyClaims.set(false);
      }
    });
  }

  setClaimantAdditionalInformation(claimId: string, information: string): void {
    this.claimantAdditionalInformation.update((values) => ({
      ...values,
      [claimId]: information
    }));
  }

  provideAdditionalInformation(claim: Claim): void {
    if (
      claim.status !== 2 ||
      this.providingInformationClaimId() ||
      this.isSearchingMyClaims()
    ) {
      return;
    }

    const additionalInformation =
      this.claimantAdditionalInformation()[claim.id]?.trim();
    const performedBy = this.myClaimsEmail().trim();

    this.informationProvidedMessages.update((messages) => ({
      ...messages,
      [claim.id]: ''
    }));
    this.informationProvidedErrors.update((errors) => ({
      ...errors,
      [claim.id]: ''
    }));

    if (!additionalInformation) {
      this.informationProvidedErrors.update((errors) => ({
        ...errors,
        [claim.id]: 'Please enter the additional information requested.'
      }));
      return;
    }

    if (!performedBy) {
      this.informationProvidedErrors.update((errors) => ({
        ...errors,
        [claim.id]: 'Enter the email address used to search for this claim.'
      }));
      return;
    }

    if (additionalInformation.length > 2000) {
      this.informationProvidedErrors.update((errors) => ({
        ...errors,
        [claim.id]: 'Additional information must be 2000 characters or fewer.'
      }));
      return;
    }

    this.providingInformationClaimId.set(claim.id);

    this.claimService.provideInformation(claim.id, {
      additionalInformation,
      performedBy
    }).subscribe({
      next: (response) => {
        this.informationProvidedMessages.update((messages) => ({
          ...messages,
          [claim.id]: response.message
        }));
        this.providingInformationClaimId.set(null);
        this.searchMyClaims();
      },
      error: (error) => {
        console.error('Failed to provide additional information:', error);
        this.informationProvidedErrors.update((errors) => ({
          ...errors,
          [claim.id]:
            error.error?.message ??
            'Unable to submit additional information. Please try again.'
        }));
        this.providingInformationClaimId.set(null);
      }
    });
  }

  get totalEstimatedAmount(): number {
    return this.claims().reduce(
      (total, claim) => total + claim.estimatedAmount,
      0
    );
  }

  private isValidEmail(email: string): boolean {
    return new FormControl(email, Validators.email).valid;
  }

getClaimTypeLabel(claimType: number): string {
  switch (claimType) {
    case 0:
      return 'Motor';
    case 1:
      return 'Property';
    default:
      return 'Unknown';
  }
}

getStatusLabel(status: number): string {
  const statuses: Record<number, string> = {
    0: 'Submitted',
    1: 'Under Review',
    2: 'Information Requested',
    3: 'Information Provided',
    4: 'Approved',
    5: 'Rejected',
    6: 'Settled'
  };

  return statuses[status] ?? 'Unknown';
}

getStatusCssClass(status: number): string {
  const classes: Record<number, string> = {
    0: 'submitted',
    1: 'under-review',
    2: 'information-requested',
    3: 'information-provided',
    4: 'approved',
    5: 'rejected',
    6: 'settled'
  };

  return classes[status] ?? 'unknown';
}

submitClaim(): void {
  if (this.isSubmitting()) {
    return;
  }

  this.submitMessage.set('');
  this.submitError.set('');

  const claimantName = this.claimForm.claimantName.trim();
  const claimantEmail = this.claimForm.claimantEmail.trim();
  const market = this.claimForm.market.trim();
  const incidentDescription = this.claimForm.incidentDescription.trim();
  const estimatedAmount = this.claimForm.estimatedAmount;

  if (!claimantName || claimantName.length > 150) {
    this.submitError.set('Please enter a claimant name of 150 characters or fewer.');
    return;
  }

  if (
    !claimantEmail ||
    !this.isValidEmail(claimantEmail) ||
    claimantEmail.length > 256
  ) {
    this.submitError.set('Please enter a valid email address of 256 characters or fewer.');
    return;
  }

  if (!market || market.length > 50) {
    this.submitError.set('Please select a valid market.');
    return;
  }

  if (this.claimForm.claimType !== 0 && this.claimForm.claimType !== 1) {
    this.submitError.set('Please select a valid claim type.');
    return;
  }

  if (
    !this.claimForm.incidentDate ||
    Number.isNaN(Date.parse(`${this.claimForm.incidentDate}T00:00:00`)) ||
    this.claimForm.incidentDate > this.today
  ) {
    this.submitError.set('Please select the incident date.');
    return;
  }

  if (
    estimatedAmount === null ||
    !Number.isFinite(estimatedAmount) ||
    estimatedAmount <= 0
  ) {
    this.submitError.set('Please enter an estimated amount greater than zero.');
    return;
  }

  if (incidentDescription.length < 10 || incidentDescription.length > 2000) {
    this.submitError.set(
      'Enter an incident description between 10 and 2000 characters.'
    );
    return;
  }

  this.isSubmitting.set(true);

  const payload = {
    ...this.claimForm,
    claimantName,
    claimantEmail,
    market,
    incidentDescription,
    estimatedAmount,
    incidentDate: new Date(
      `${this.claimForm.incidentDate}T12:00:00`
    ).toISOString()
  };

  this.claimService.createClaim(payload).subscribe({
    next: (claim) => { console.log('API response:', claim);
      this.submitMessage.set(
        `Claim submitted successfully! Your claim number is ${claim.claimNumber}.`
      );

      this.claimForm = {
        claimantName: '',
        claimantEmail: '',
        market: 'Malaysia',
        claimType: 0,
        incidentDate: '',
        incidentDescription: '',
        estimatedAmount: null
      };

      this.isSubmitting.set(false);
      this.loadDashboard();
    },
    error: (error) => {
      console.error('Failed to submit claim:', error);

      this.submitError.set(
        error.error?.message ??
        'Unable to submit your claim. Please check that the API is running.'
      );

      this.isSubmitting.set(false);
    }
  });
}

}
