import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';

import { Category } from '../models/category';

import { CategoryService } from '../services/category.service';

import { NewCategoryDialogComponent } from '../new-category-dialog/new-category-dialog.component';

@Component({
	selector: 'app-categories',
	templateUrl: './categories.component.html',
	styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {
	@ViewChild(MatSort, { static: false }) sort: MatSort;

	@Input() authenticationToken: string;

	displayedColumns = ['select', 'name', 'productsCount'];

	dataSource: MatTableDataSource<Category>;
	selection = new SelectionModel<Category>(true, []);

	dataRetrievalError: string;

	constructor(
		private categoryService: CategoryService,
		private newCategoryDialog: MatDialog) {
	}

	ngOnInit() {
		this.categoryService.getCategories(this.authenticationToken).subscribe(data => {
			this.dataSource = new MatTableDataSource(data);

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

	applyFilter(term: string) {
		this.dataSource.filter = term.trim().toLowerCase();
	}

	clearFilter(filter: HTMLInputElement) {
		this.dataSource.filter = filter.value = '';
	}

	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.dataSource.data.length;
		return numSelected === numRows;
	}

	masterToggle() {
		this.isAllSelected() ?
			this.selection.clear() :
			this.dataSource.data.forEach(row => this.selection.select(row));
	}

	checkboxLabel(row?: Category): string {
		if (!row) {
			return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
		}

		return `${this.selection.isSelected(row) ? 'deselect' : 'select'} ${row.name}`;
	}

	toggleRowSelection(row: Category) {
		if (this.selection.isSelected(row)) {
			this.selection.deselect(row);
		} else {
			this.selection.select(row);
		}
	}

	addNewCategory(): void {
		const dialogRef = this.newCategoryDialog.open(NewCategoryDialogComponent, {
			data: this.authenticationToken
		});

		dialogRef.afterClosed().subscribe(result => {
			if (result as Category) {
				this.dataSource.data.push(result);
				this.dataSource.filter = this.dataSource.filter;
			}
		});
	}
}