import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';

import { Category } from '../models/category';

const SAMPLE_DATA: Category[] = [
	{ id: "00000000-0000-0000-0000-000000000000", name: "Default", productsCount: 5 },
	{ id: "36b0fa5b-7d41-4ce2-b3b8-add926a380cf", name: "Guitars", productsCount: 17 },
	{ id: "af39e9a4-1a74-4122-b5fd-08d6e9baa407", name: "Motorcycles", productsCount: 3 },
	{ id: "c5178f5b-bb83-4c0b-8589-927275d8bf71", name: "Nothing", productsCount: 0 },
];

@Component({
	selector: 'app-categories',
	templateUrl: './categories.component.html',
	styleUrls: ['./categories.component.css']
})
export class CategoriesComponent implements OnInit {
	@ViewChild(MatSort, { static: true }) sort: MatSort;

	displayedColumns = ['select', 'name', 'productsCount'];

	dataSource = new MatTableDataSource(SAMPLE_DATA);
	selection = new SelectionModel<Category>(true, []);

	ngOnInit() {
		this.dataSource.sort = this.sort;
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
}