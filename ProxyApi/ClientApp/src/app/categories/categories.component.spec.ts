import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { Observable } from 'rxjs';

import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';

import { CategoriesComponent } from './categories.component';

import { Category } from '../models/category';

import { CategoryService } from '../services/category.service';

describe('CategoriesComponent', () => {
	let component: CategoriesComponent;
	let categoryService: CategoryService;
	let fixture: ComponentFixture<CategoriesComponent>;

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
				MatSortModule,
				MatTableModule,
			],
			declarations: [
				CategoriesComponent,
			],
		}).compileComponents();
	}));

	beforeEach(() => {
		categoryService = TestBed.get(CategoryService);

		spyOn(categoryService, 'getCategories').and.returnValue(new Observable<Category[]>());

		fixture = TestBed.createComponent(CategoriesComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should compile', () => {
		expect(component).toBeTruthy();
	});
});