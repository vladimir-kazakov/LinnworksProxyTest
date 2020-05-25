import { Component, Inject } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { CategoryService } from '../services/category.service';

@Component({
	selector: 'app-new-category-dialog',
	templateUrl: './new-category-dialog.component.html',
	styleUrls: ['./new-category-dialog.component.css']
})
export class NewCategoryDialogComponent {
	categoryNameValidators = [
		Validators.required,
	];

	newCategoryName = new FormControl('', this.categoryNameValidators);

	private error: string;

	constructor(
		private categoryService: CategoryService,
		private dialogRef: MatDialogRef<NewCategoryDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public authenticationToken: string) {
	}

	addNewCategory(): void {
		this.categoryService.addCategory(this.authenticationToken, this.newCategoryName.value).subscribe(newCategory => {
			this.dialogRef.close(newCategory);
		}, error => {
			this.newCategoryName.setErrors({ 'incorrect': true });

			if (error.error.Message) {
				this.error = error.error.Message;
			}
		});
	}

	getErrorMessage(): string {
		if (this.newCategoryName.hasError('required')) {
			return 'Provide a value.';
		}

		if (this.newCategoryName.hasError('incorrect')) {
			return this.error ? this.error : 'Invalid or duplicate value.';
		}

		return '';
	}
}