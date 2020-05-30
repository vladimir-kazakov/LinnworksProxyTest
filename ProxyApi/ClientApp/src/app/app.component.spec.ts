import { TestBed, async, ComponentFixture } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { AppComponent } from './app.component';
import { CategoriesComponent } from './categories/categories.component';

import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';

describe('AppComponent', () => {
	let sut: AppComponent;

	let fixture: ComponentFixture<AppComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			imports: [
				HttpClientModule,
				NoopAnimationsModule,
				MatCheckboxModule,
				MatDialogModule,
				MatFormFieldModule,
				MatIconModule,
				MatInputModule,
				MatTableModule,
			],
			declarations: [
				AppComponent,
				CategoriesComponent,
			],
		}).compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(AppComponent);

		sut = fixture.componentInstance;

		fixture.detectChanges();
	});

	describe('Component', () => {
		it('#isUserLoggedIn when user is not logged in returns false', () => {
			sut.authenticationToken = null;

			const actual = sut.isUserLoggedIn();

			expect(actual).toBeFalsy();
		});

		it('#isUserLoggedIn when user is logged in returns true', () => {
			sut.authenticationToken = 'test';

			const actual = sut.isUserLoggedIn();

			expect(actual).toBeTruthy();
		});

		it('#isDemoUser when user is demo user returns true', () => {
			sut.authenticationToken = '00000000-0000-0000-0000-000000000000';

			const actual = sut.isDemoUser();

			expect(actual).toBeTruthy();
		});

		it('#isDemoUser when user is normal user returns false', () => {
			sut.authenticationToken = 'test';

			const actual = sut.isDemoUser();

			expect(actual).toBeFalsy();
		});
	});

	describe('HTML', () => {
		it('should show navigation bar with a title', () => {
			const actual: HTMLAnchorElement = fixture.debugElement.query(
				By.css('.navbar > a.navbar-brand')).nativeElement;

			expect(actual.text.toLowerCase()).toContain('categories');
		});

		it('should require authentication by default', () => {
			const actualCard: HTMLDivElement = fixture.debugElement.query(
				By.css('.card-authentication > .card-header')).nativeElement;

			const actualButton: HTMLButtonElement = fixture.debugElement.query(
				By.css('.card-authentication button')).nativeElement;

			expect(actualCard.innerText.toLowerCase()).toBe('authentication required');
			expect(actualButton.innerText.toLowerCase()).toBe('login');
		});

		it('should not require authentication when user is logged in', () => {
			sut.authenticationToken = 'test';

			fixture.detectChanges();

			const actual = fixture.debugElement.query(By.css('.card-authentication'));

			expect(actual).toBeFalsy();
		});

		it('should not show logged in user by default', () => {
			const actual = fixture.debugElement.query(By.css('.login-info'));

			expect(actual).toBeFalsy();
		});

		it('should show logged in user when user is logged in', () => {
			const expectedAuthenticationToken = 'test';

			sut.authenticationToken = expectedAuthenticationToken;

			fixture.detectChanges();

			const actual: HTMLDivElement = fixture.debugElement.query(
				By.css('.login-info')).nativeElement;

			expect(actual.innerText).toContain(expectedAuthenticationToken);
		});

		it('should show logout link when user is logged in', () => {
			sut.authenticationToken = 'test';

			fixture.detectChanges();

			const actual: HTMLAnchorElement = fixture.debugElement.query(
				By.css('.login-info > a')).nativeElement;

			expect(actual.text.toLowerCase()).toBe('logout');
		});

		it('should not show categories by default', () => {
			const actual = fixture.debugElement.query(By.css('app-categories'));

			expect(actual).toBeFalsy();
		});

		it('should show categories when user is logged in', () => {
			sut.authenticationToken = 'test';

			fixture.detectChanges();

			const actual = fixture.debugElement.query(By.css('app-categories'));

			expect(actual).toBeTruthy();
		});

		it('should not show a demo user alert by default', () => {
			const actual = fixture.debugElement.query(By.css('.alert'));

			expect(actual).toBeFalsy();
		});

		it('should not show a demo user alert when user is logged in as normal user', () => {
			sut.authenticationToken = '406e9609-7e40-4415-b7d7-ae52a22b0158';

			fixture.detectChanges();

			const actual = fixture.debugElement.query(By.css('.alert'));

			expect(actual).toBeFalsy();
		});

		it('should show a demo user alert when user is logged in as demo user', () => {
			sut.authenticationToken = '00000000-0000-0000-0000-000000000000';

			fixture.detectChanges();

			const actual: HTMLDivElement = fixture.debugElement.query(By.css('.alert')).nativeElement;

			expect(actual.innerText.toLowerCase()).toContain('demo user');
		});

		it('should show login dialog when login button is clicked', () => {
			spyOn(sut, 'login');

			const loginButton = fixture.debugElement.query(By.css('.card-authentication button'));

			loginButton.triggerEventHandler('click', null);

			expect(sut.login).toHaveBeenCalled();
		});

		it('should log out user when logout link is clicked', () => {
			spyOn(sut, 'logout');

			sut.authenticationToken = 'test';

			fixture.detectChanges();

			const logoutLink = fixture.debugElement.query(By.css('.login-info > a'));

			logoutLink.triggerEventHandler('click', null);

			expect(sut.logout).toHaveBeenCalled();
		});
	});
});