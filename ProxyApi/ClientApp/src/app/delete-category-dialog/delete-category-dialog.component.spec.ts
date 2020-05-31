import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Observable } from 'rxjs';

import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DeleteCategoryDialogComponent, DeleteCategoryDialogData } from './delete-category-dialog.component';

import { CategoryService } from '../services/category.service';

describe('DeleteCategoryDialogComponent', () => {
	let sut: DeleteCategoryDialogComponent;

	let fixture: ComponentFixture<DeleteCategoryDialogComponent>;
	let categoryService: CategoryService;

	const expectedDialogData: DeleteCategoryDialogData = {
		authenticationToken: 'token',
		categories: [
			{ id: 'one', name: 'first', productsCount: 1 },
			{ id: 'two', name: 'second', productsCount: 2 },
		],
	};

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			imports: [
				HttpClientTestingModule,
				MatDialogModule,
			],
			declarations: [
				DeleteCategoryDialogComponent,
			],
			providers: [
				CategoryService,
				{ provide: MatDialogRef, useValue: jasmine.createSpyObj('dialog', ['close']) },
				{ provide: MAT_DIALOG_DATA, useValue: expectedDialogData },
			],
		}).compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(DeleteCategoryDialogComponent);
		categoryService = TestBed.get(CategoryService);

		sut = fixture.componentInstance;

		fixture.detectChanges();
	});

	describe('Component', () => {
		it('#deleteCategories should delete all selected categories', () => {
			spyOn(categoryService, 'deleteCategory').and.returnValue(new Observable());

			sut.deleteCategories();

			expect(categoryService.deleteCategory).toHaveBeenCalledTimes(expectedDialogData.categories.length);
		});

		it('#deleteCategories when error happens memorizes it', () => {
			spyOn(categoryService, 'deleteCategory').and.returnValue(new Observable(observer => {
				observer.error({
					error: { Message:'test' }
				});
			}));

			sut.deleteCategories();

			expect(sut.errors.length).toBe(expectedDialogData.categories.length);
		});
	});

	describe('HTML', () => {
		it('should have proper title', () => {
			const actual: HTMLHeadingElement = fixture.debugElement.query(By.css('h2')).nativeElement;

			expect(actual.innerText.toLowerCase()).toBe('delete categories');
		});

		it('should ask for confirmation to delete selected categories', () => {
			let actualContent = fixture.debugElement.query(By.css('mat-dialog-content'));
			let actualCategories = fixture.debugElement.queryAll(By.css('mat-dialog-content li'));

			expect(actualContent.nativeElement.textContent.toLowerCase()).toContain('are you sure');
			expect(actualCategories.length).toBe(expectedDialogData.categories.length);
		});

		it('should show errors with reasons why categories cannot be deleted', () => {
			const expectedErrorMessages = ['something', 'bad', 'happened'];

			sut.errors = expectedErrorMessages;

			fixture.detectChanges();

			const actual = fixture.debugElement.queryAll(By.css('.alert-danger'));

			expect(actual.length).toBe(expectedErrorMessages.length);
			expect(actual.map(e => e.nativeElement.textContent.trim())).toEqual(expectedErrorMessages);
		});

		it('should have cancel button', () => {
			const actual = fixture.debugElement.queryAll(By.css('button'))
				.find(b => b.nativeElement.textContent.trim().toLowerCase() == 'cancel');

			expect(actual).toBeTruthy();
		});

		it('should have delete button', () => {
			spyOn(sut, 'deleteCategories');

			const actual = fixture.debugElement.queryAll(By.css('button'))
				.find(b => b.nativeElement.textContent.trim().toLowerCase() == 'delete');

			expect(actual).toBeTruthy();

			actual.triggerEventHandler('click', null);

			expect(sut.deleteCategories).toHaveBeenCalled();
		})
	});
});