import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { Category } from '../models/category';

import { CategoryService } from './category.service';

describe('CategoryService', () => {
	let sut: CategoryService;

	let httpTestingController: HttpTestingController;

	const expectedUrlPath = '/categories';

	beforeEach(() => {
		TestBed.configureTestingModule({
			imports: [HttpClientTestingModule]
		});

		sut = TestBed.get(CategoryService);

		httpTestingController = TestBed.get(HttpTestingController);
	});

	afterEach(() => {
		httpTestingController.verify();
	});

	it('#addCategory should send a proper POST request', () => {
		const expectedAuthenticationToken = 'token';

		const expectedCategory: Category = {
			id: '43a28965-cc57-4552-b112-5f50679af631',
			name: 'New',
			productsCount: 0,
		};

		sut.addCategory(expectedAuthenticationToken, expectedCategory.name).subscribe(actualCategory => {
			expect(actualCategory).toEqual(expectedCategory);
		}, fail);

		var actual = httpTestingController.expectOne(expectedUrlPath);

		expect(actual.request.method).toBe('POST');
		expect(actual.request.responseType).toBe('json');
		expect(actual.request.headers.get('Authorization')).toBe(expectedAuthenticationToken);
		expect(actual.request.body).toEqual({ name: expectedCategory.name });

		actual.flush(expectedCategory);
	});

	it('#getCategories should send a proper GET request', () => {
		const expectedAuthenticationToken = 'token';

		const expectedCategories: Category[] = [
			{ id: 'one', name: 'first', productsCount: 1 },
			{ id: 'two', name: 'second', productsCount: 2 },
		];

		sut.getCategories(expectedAuthenticationToken).subscribe(actualCategories => {
			expect(actualCategories).toEqual(expectedCategories);
		}, fail);

		var actual = httpTestingController.expectOne(expectedUrlPath);

		expect(actual.request.method).toBe('GET');
		expect(actual.request.responseType).toBe('json');
		expect(actual.request.headers.get('Authorization')).toBe(expectedAuthenticationToken);
		expect(actual.request.body).toBeNull();

		actual.flush(expectedCategories);
	});

	it('#updateCategory should send a proper PUT request', () => {
		const expectedAuthenticationToken = 'token';
		const expectedData = {};

		const updatedCategory: Category = {
			id: '13682796-8135-4d7f-aaba-6b6ec3688d80',
			name: 'Updated',
			productsCount: 4,
		};

		sut.updateCategory(expectedAuthenticationToken, updatedCategory).subscribe(actualData => {
			expect(actualData).toEqual(expectedData);
		}, fail);

		var actual = httpTestingController.expectOne(`${expectedUrlPath}/${updatedCategory.id}`);

		expect(actual.request.method).toBe('PUT');
		expect(actual.request.headers.get('Authorization')).toBe(expectedAuthenticationToken);
		expect(actual.request.body).toEqual(updatedCategory);

		actual.flush(expectedData);
	});

	it('#deleteCategory should send a proper DELETE request', () => {
		const expectedAuthenticationToken = 'token';
		const expectedData = {};

		const idOfCategoryToDelete = '8c571c3d-7e87-4b5c-b9d7-7ab986e703a9';

		sut.deleteCategory(expectedAuthenticationToken, idOfCategoryToDelete).subscribe(actualData => {
			expect(actualData).toEqual(expectedData);
		}, fail);

		var actual = httpTestingController.expectOne(`${expectedUrlPath}/${idOfCategoryToDelete}`);

		expect(actual.request.method).toBe('DELETE');
		expect(actual.request.headers.get('Authorization')).toBe(expectedAuthenticationToken);
		expect(actual.request.body).toBeNull();

		actual.flush(expectedData);
	});
});