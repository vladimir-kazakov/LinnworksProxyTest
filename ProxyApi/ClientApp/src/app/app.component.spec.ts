import { TestBed, async } from '@angular/core/testing';
import { AppComponent } from './app.component';

import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { CategoriesComponent } from './categories/categories.component';

describe('AppComponent', () => {
	beforeEach(async(() => {
		TestBed.configureTestingModule({
			imports: [
				MatCheckboxModule,
				MatDialogModule,
				MatFormFieldModule,
				MatIconModule,
				MatTableModule,
			],
			declarations: [
				AppComponent,
				CategoriesComponent,
			],
		}).compileComponents();
	}));

	it('should create the app', () => {
		const fixture = TestBed.createComponent(AppComponent);
		const app = fixture.debugElement.componentInstance;
		expect(app).toBeTruthy();
	});
});