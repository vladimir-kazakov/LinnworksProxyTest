import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { retry } from 'rxjs/operators';

import { Category } from '../models/category';

@Injectable({
	providedIn: 'root'
})
export class CategoryService {
	private readonly endpointUrl = '/categories';
	private readonly retryCount = 3;

	constructor(private httpClient: HttpClient) { }

	addCategory(authenticationToken: string, categoryName: string): Observable<Category> {
		return this.httpClient.post<Category>(this.endpointUrl, { name: categoryName }, {
			headers: new HttpHeaders({ 'Authorization': authenticationToken }),
			responseType: 'json',
		}).pipe(retry(this.retryCount));
	}

	getCategories(authenticationToken: string): Observable<Category[]> {
		return this.httpClient.get<Category[]>(this.endpointUrl, {
			headers: new HttpHeaders({ 'Authorization': authenticationToken }),
			responseType: 'json',
		}).pipe(retry(this.retryCount));
	}

	updateCategory(authenticationToken: string, updatedCategory: Category): Observable<any> {
		const url = `${this.endpointUrl}/${updatedCategory.id}`;

		return this.httpClient.put(url, updatedCategory, {
			headers: new HttpHeaders({ 'Authorization': authenticationToken }),
		}).pipe(retry(this.retryCount));
	}

	deleteCategory(authenticationToken: string, categoryId: string): Observable<any> {
		const url = `${this.endpointUrl}/${categoryId}`;

		return this.httpClient.delete(url, {
			headers: new HttpHeaders({ 'Authorization': authenticationToken }),
		}).pipe(retry(this.retryCount));
	}
}