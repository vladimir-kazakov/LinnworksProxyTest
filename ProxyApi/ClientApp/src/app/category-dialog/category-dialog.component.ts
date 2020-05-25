import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Category } from '../models/category';

import { CategoryService } from '../services/category.service';

export interface CategoryDialogData {
	authenticationToken: string;
	action: string;
	category?: Category;
}

@Component({
	selector: 'app-category-dialog',
	templateUrl: './category-dialog.component.html',
	styleUrls: ['./category-dialog.component.css']
})
export class CategoryDialogComponent implements OnInit {
	categoryNameValidators = [
		Validators.required,
	];

	categoryName = new FormControl('', this.categoryNameValidators);

	private error: string;

	constructor(
		private categoryService: CategoryService,
		private dialogRef: MatDialogRef<CategoryDialogComponent>,
		@Inject(MAT_DIALOG_DATA) private dialogData: CategoryDialogData) {
	}

	ngOnInit() {
		if (this.dialogData.category) {
			this.categoryName.setValue(this.dialogData.category.name);
		}
	}

	performAction(): void {
		const action = this.dialogData.action.toLowerCase();

		if (action == 'add') {
			this.addCategory();
		} else if (action == 'edit') {
			this.editCategory();
		}
	}

	handleWebApiError(error): void {
		this.categoryName.setErrors({ 'incorrect': true });

		if (error.error.Message) {
			this.error = error.error.Message;
		}
	}

	addCategory(): void {
		this.categoryService.addCategory(this.dialogData.authenticationToken, this.categoryName.value).subscribe(newCategory => {
			this.dialogRef.close(newCategory);
		}, error => this.handleWebApiError(error));
	}

	editCategory(): void {
		if (!this.dialogData.category) {
			this.dialogRef.close();
			return;
		}

		const oldName = this.dialogData.category.name.toLowerCase().trim();
		const newName = this.categoryName.value.toLowerCase().trim();

		if (oldName == newName) {
			this.dialogRef.close();
			return;
		}

		const updatedCategory: Category = {
			id: this.dialogData.category.id,
			name: this.categoryName.value.trim(),
			productsCount: this.dialogData.category.productsCount,
		}

		this.categoryService.updateCategory(this.dialogData.authenticationToken, updatedCategory).subscribe(_ => {
			this.dialogRef.close(updatedCategory);
		}, error => this.handleWebApiError(error));
	}

	getErrorMessage(): string {
		if (this.categoryName.hasError('required')) {
			return 'Provide a value.';
		}

		if (this.categoryName.hasError('incorrect')) {
			return this.error ? this.error : 'Invalid or already exists.';
		}

		return '';
	}
}