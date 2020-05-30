import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule } from '@angular/forms';

import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material';
import { MatInputModule } from '@angular/material/input';

import { CategoryDialogComponent } from './category-dialog.component';

describe('CategoryDialogComponent', () => {
	let component: CategoryDialogComponent;
	let fixture: ComponentFixture<CategoryDialogComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			imports: [
				NoopAnimationsModule,
				HttpClientModule,
				ReactiveFormsModule,
				MatDialogModule,
				MatFormFieldModule,
				MatInputModule,
			],
			declarations: [
				CategoryDialogComponent,
			],
			providers: [
				{ provide: MatDialogRef, useValue: {} },
				{ provide: MAT_DIALOG_DATA, useValue: {} },
			],
		}).compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(CategoryDialogComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});