import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Subject } from 'rxjs';
import { App } from './app';
import { ClaimService, DashboardData } from './claim.service';

describe('App', () => {
  let fixture: ComponentFixture<App>;
  let dashboardResponse: Subject<DashboardData>;

  beforeEach(async () => {
    dashboardResponse = new Subject<DashboardData>();

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter([]),
        {
          provide: ClaimService,
          useValue: {
            getDashboard: () => dashboardResponse.asObservable()
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    fixture.detectChanges();
  });

  it('shows a loading state while the dashboard request is pending', () => {
    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.textContent).toContain(
      'Loading dashboard data from the claims API...'
    );
  });

  it('renders dashboard data when the API request resolves', async () => {
    dashboardResponse.next({
      totalClaims: 2,
      submittedClaims: 0,
      underReviewClaims: 0,
      informationRequestedClaims: 0,
      informationProvidedClaims: 0,
      approvedClaims: 0,
      rejectedClaims: 1,
      settledClaims: 1,
      outstandingLiability: 0,
      officerWorkloads: [],
      recentClaims: []
    });

    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).not.toContain(
      'Loading dashboard data from the claims API...'
    );
    expect(compiled.querySelector('.stats-grid .stat-value')?.textContent).toBe(
      '2'
    );
  });
});
