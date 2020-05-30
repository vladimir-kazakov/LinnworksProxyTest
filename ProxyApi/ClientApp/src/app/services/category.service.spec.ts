import { TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';

import { CategoryService } from './category.service';

describe('CategoryService', () => {
	beforeEach(() => TestBed.configureTestingModule({
		imports: [HttpClientModule]
	}));

	it('should be created', () => {
		const service: CategoryService = TestBed.get(CategoryService);
		expect(service).toBeTruthy();
	});
});