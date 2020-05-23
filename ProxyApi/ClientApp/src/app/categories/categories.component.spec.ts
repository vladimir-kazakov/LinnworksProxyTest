import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';

import { CategoriesComponent } from './categories.component';

describe('CategoriesComponent', () => {
	let component: CategoriesComponent;
	let fixture: ComponentFixture<CategoriesComponent>;

	beforeEach(async(() => {
		TestBed.configureTestingModule({
			declarations: [CategoriesComponent],
			imports: [
				NoopAnimationsModule,
				MatPaginatorModule,
				MatSortModule,
				MatTableModule,
			]
		}).compileComponents();
	}));

	beforeEach(() => {
		fixture = TestBed.createComponent(CategoriesComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should compile', () => {
		expect(component).toBeTruthy();
	});
});