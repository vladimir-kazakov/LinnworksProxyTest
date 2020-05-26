import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Category } from '../models/category';

import { CategoryService } from '../services/category.service';

export interface DeleteCategoryDialogData {
	authenticationToken: string;
	categories: Category[];
}

@Component({
	selector: 'app-delete-category-dialog',
	templateUrl: './delete-category-dialog.component.html',
	styleUrls: ['./delete-category-dialog.component.css']
})
export class DeleteCategoryDialogComponent {
	errors: string[];

	constructor(
		private categoryService: CategoryService,
		private dialogRef: MatDialogRef<DeleteCategoryDialogComponent>,
		@Inject(MAT_DIALOG_DATA) private dialogData: DeleteCategoryDialogData) {
	}

	deleteCategories(): void {
		this.errors = [];

		for (const category of this.dialogData.categories) {
			this.categoryService.deleteCategory(this.dialogData.authenticationToken, category.id).subscribe(_ => {
			}, error => {
				if (error.error.Message) {
					this.errors.push(error.error.Message);
				}
			}).add(() => {
				if (this.errors.length < 1) {
					this.dialogRef.close(this.dialogData.categories);
				}
			});
		}
	}
}