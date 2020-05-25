import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { retry } from 'rxjs/operators';

import { Category } from '../models/category';

@Injectable({
	providedIn: 'root'
})
export class CategoryService {
	constructor(private httpClient: HttpClient) { }

	getCategories(authenticationToken: string): Observable<Category[]> {
		return this.httpClient.get<Category[]>('/categories', {
			headers: new HttpHeaders({
				'Authorization': authenticationToken
			}),
			responseType: 'json'
		}).pipe(retry(3));
	}
}