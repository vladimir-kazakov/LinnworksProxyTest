import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';

import { Category } from '../models/category';

import { CategoryService } from '../services/category.service';

import { CategoryDialogAction, CategoryDialogComponent, CategoryDialogData } from '../category-dialog/category-dialog.component';
import { DeleteCategoryDialogComponent, DeleteCategoryDialogData } from '../delete-category-dialog/delete-category-dialog.component';

@Component({
	selector: 'app-categories',
	templateUrl: './categories.component.html',
	styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {
	@ViewChild(MatSort, { static: false }) sort: MatSort;

	@Input() authenticationToken: string;

	displayedColumns = ['select', 'name', 'editName', 'productsCount'];

	dataSource: MatTableDataSource<Category>;
	selection = new SelectionModel<Category>(true, []);

	dataRetrievalError: string;

	constructor(
		private categoryService: CategoryService,
		private dialog: MatDialog) {
	}

	ngOnInit() {
		this.categoryService.getCategories(this.authenticationToken).subscribe(data => {
			this.dataSource = new MatTableDataSource(data);

			this.dataSource.filterPredicate = (category, filter) => {
				var categoryNameMatches = category.name.toLowerCase().indexOf(filter.toLowerCase()) > -1;
				var productsCountMatches = category.productsCount.toString().indexOf(filter) > -1;

				return categoryNameMatches || productsCountMatches;
			};

			this.dataSource.sort = this.sort;
		}, error => {
			this.dataRetrievalError =
				`HTTP status code: ${error.status} (${error.statusText}).`;

			if (error.error.Message) {
				this.dataRetrievalError += ` Reason: ${error.error.Message}`;
			}

			if (error.status == 401) {
				this.dataRetrievalError += ' Try using a different authentication token.';
			}
		});
	}

	isDefault(category: Category): boolean {
		return category.name.toLowerCase() == 'default';
	}

	applyFilter(term: string): void {
		this.dataSource.filter = term.trim().toLowerCase();
	}

	clearFilter(filter: HTMLInputElement): void {
		this.dataSource.filter = filter.value = '';
	}

	isAllSelected(): boolean {
		const selectedCount = this.selection.selected.length;
		const selectableCount = this.dataSource.data.filter(c => !this.isDefault(c)).length;
		return selectedCount == selectableCount;
	}

	isAnythingSelected(): boolean {
		return this.selection.selected.length > 0;
	}

	masterToggle(): void {
		if (this.isAllSelected()) {
			this.selection.clear();
		} else {
			this.dataSource.data.forEach(c => {
				if (!this.isDefault(c)) {
					this.selection.select(c);
				}
			})
		}
	}

	checkboxLabel(row?: Category): string {
		if (!row) {
			return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
		}

		return `${this.selection.isSelected(row) ? 'deselect' : 'select'} ${row.name}`;
	}

	addCategory(): void {
		const dialogData: CategoryDialogData = {
			action: CategoryDialogAction.Add,
			authenticationToken: this.authenticationToken,
		};

		const dialogRef = this.dialog.open(CategoryDialogComponent, { data: dialogData });

		dialogRef.afterClosed().subscribe(result => {
			if (result as Category) {
				this.dataSource.data.push(result);
				this.dataSource.filter = this.dataSource.filter;
			}
		});
	}

	editCategory(category: Category): void {
		const dialogData: CategoryDialogData = {
			action: CategoryDialogAction.Edit,
			authenticationToken: this.authenticationToken,
			category: category,
		};

		const dialogRef = this.dialog.open(CategoryDialogComponent, { data: dialogData });

		dialogRef.afterClosed().subscribe(result => {
			if (result as Category) {
				category.name = result.name;
				this.dataSource.filter = this.dataSource.filter;
			}
		});
	}

	deleteSelectedCategories(): void {
		const dialogData: DeleteCategoryDialogData = {
			authenticationToken: this.authenticationToken,
			categories: this.selection.selected.filter(c => !this.isDefault(c)).sort((a, b) => {
				return a.name.localeCompare(b.name);
			}),
		};

		const dialogRef = this.dialog.open(DeleteCategoryDialogComponent, { data: dialogData });

		dialogRef.afterClosed().subscribe(result => {
			if (result as Category[]) {
				result.forEach(c => {
					const index = this.dataSource.data.findIndex(i => i.name == c.name);

					if (index > -1) {
						this.dataSource.data.splice(index, 1);
					}
				});

				this.selection.clear();
				this.dataSource.filter = this.dataSource.filter;
			}
		});
	}
}