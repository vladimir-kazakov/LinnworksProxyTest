import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';

import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material';
import { MatInputModule } from '@angular/material/input';

import { LoginDialogComponent } from './login-dialog.component';

describe('LoginDialogComponent', () => {
	let component: LoginDialogComponent;
	let fixture: ComponentFixture<LoginDialogComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			imports: [
				NoopAnimationsModule,
				ReactiveFormsModule,
				MatDialogModule,
				MatFormFieldModule,
				MatInputModule,
			],
			declarations: [
				LoginDialogComponent,
			],
		}).compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(LoginDialogComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});