import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { LoginDialogComponent } from './login-dialog/login-dialog.component';

@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
	private readonly authenticationTokenStorageKey = "authenticationToken";

	title = 'Linnworks Categories';
	authenticationToken: string;

	constructor(private loginDialog: MatDialog) { }

	ngOnInit(): void {
		this.authenticationToken = sessionStorage.getItem(this.authenticationTokenStorageKey);
	}

	isUserLoggedIn(): boolean {
		return this.authenticationToken != null && this.authenticationToken.trim() != '';
	}

	isDemoUser(): boolean {
		return this.authenticationToken == '00000000-0000-0000-0000-000000000000';
	}

	login(): void {
		const dialogRef = this.loginDialog.open(LoginDialogComponent);

		dialogRef.afterClosed().subscribe(result => {
			this.authenticationToken = result;

			sessionStorage.setItem(this.authenticationTokenStorageKey, this.authenticationToken);
		});
	}

	logout(event: Event): void {
		event.preventDefault();

		this.authenticationToken = null;

		sessionStorage.removeItem(this.authenticationTokenStorageKey);
	}
}