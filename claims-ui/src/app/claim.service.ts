
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Claim {
  id: string;
  claimNumber: string;
  claimantName: string;
  claimantEmail: string;
  market: string;
  claimType: number;
  incidentDate: string;
  incidentDescription: string;
  estimatedAmount: number;
  status: number;
  assignedOfficer: string | null;
  additionalInformation: string | null;
  decisionReason: string | null;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface OfficerWorkload {
  officerName: string;
  assignedClaims: number;
}

export interface DashboardData {
  totalClaims: number;
  submittedClaims: number;
  underReviewClaims: number;
  informationRequestedClaims: number;
  informationProvidedClaims: number;
  approvedClaims: number;
  rejectedClaims: number;
  settledClaims: number;
  outstandingLiability: number;
  officerWorkloads: OfficerWorkload[];
  recentClaims: Claim[];
  
}

export interface AssignClaimRequest {
  assignedOfficer: string;
  performedBy: string;
}

export interface RequestInformationRequest {
  informationRequest: string;
  performedBy: string;
}

export interface ProvideInformationRequest {
  additionalInformation: string;
  performedBy: string;
}

export interface ClaimDecisionRequest {
  decisionReason: string;
  performedBy: string;
}

export interface SettleClaimRequest {
  performedBy: string;
}

export interface ClaimActionResponse {
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClaimService {
  private readonly apiUrl = 'http://localhost:5192/api';

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardData> {
    return this.http.get<DashboardData>(
      `${this.apiUrl}/dashboard`
    );
  }

  getClaims(claimantEmail?: string): Observable<Claim[]> {
    let params = new HttpParams();

    if (claimantEmail?.trim()) {
      params = params.set('claimantEmail', claimantEmail);
    }

    return this.http.get<Claim[]>(
      `${this.apiUrl}/Claims`,
      { params }
    );
  }

  trackClaim(claimNumber: string): Observable<Claim> {
    return this.http.get<Claim>(
      `${this.apiUrl}/Claims/track/${encodeURIComponent(claimNumber)}`
    );
  }

  assignClaim(
    id: string,
    request: AssignClaimRequest
  ): Observable<ClaimActionResponse> {
    return this.http.post<ClaimActionResponse>(
      `${this.apiUrl}/Claims/${encodeURIComponent(id)}/assign`,
      request
    );
  }

  requestInformation(
    id: string,
    request: RequestInformationRequest
  ): Observable<ClaimActionResponse> {
    return this.http.post<ClaimActionResponse>(
      `${this.apiUrl}/Claims/${encodeURIComponent(id)}/request-information`,
      request
    );
  }

  provideInformation(
    id: string,
    request: ProvideInformationRequest
  ): Observable<ClaimActionResponse> {
    return this.http.post<ClaimActionResponse>(
      `${this.apiUrl}/Claims/${encodeURIComponent(id)}/provide-information`,
      request
    );
  }

  approveClaim(
    id: string,
    request: ClaimDecisionRequest
  ): Observable<ClaimActionResponse> {
    return this.http.post<ClaimActionResponse>(
      `${this.apiUrl}/Claims/${encodeURIComponent(id)}/approve`,
      request
    );
  }

  rejectClaim(
    id: string,
    request: ClaimDecisionRequest
  ): Observable<ClaimActionResponse> {
    return this.http.post<ClaimActionResponse>(
      `${this.apiUrl}/Claims/${encodeURIComponent(id)}/reject`,
      request
    );
  }

  settleClaim(
    id: string,
    request: SettleClaimRequest
  ): Observable<ClaimActionResponse> {
    return this.http.post<ClaimActionResponse>(
      `${this.apiUrl}/Claims/${encodeURIComponent(id)}/settle`,
      request
    );
  }

  createClaim(claim: {
  claimantName: string;
  claimantEmail: string;
  market: string;
  claimType: number;
  incidentDate: string;
  incidentDescription: string;
  estimatedAmount: number;
}): Observable<Claim> {
  return this.http.post<Claim>(
    `${this.apiUrl}/Claims`,
    claim
  );
}
}
