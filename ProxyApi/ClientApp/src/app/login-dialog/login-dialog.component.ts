import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

@Component({
	selector: 'app-login-dialog',
	templateUrl: './login-dialog.component.html',
	styleUrls: ['./login-dialog.component.css']
})
export class LoginDialogComponent {
	authenticationTokenValidators = [
		Validators.required,
		Validators.pattern('[0-9A-Fa-f]{8}-([0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}'),
	];

	authenticationToken = new FormControl('', this.authenticationTokenValidators);

	getErrorMessage(): string {
		if (this.authenticationToken.hasError('required')) {
			return 'Provide a value.';
		}

		if (this.authenticationToken.hasError('pattern')) {
			return 'Invalid value.';
		}

		return '';
	}
}