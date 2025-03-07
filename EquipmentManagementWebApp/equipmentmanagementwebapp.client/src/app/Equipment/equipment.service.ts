import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Agora a interface `Equipment` está sendo exportada
export interface Equipment {
  id: number;
  name: string;
  installation: string;
  batch: string;
  operator: string;
  manufacturer: string;
  model: string;
  version: string;
  purchaseDate: string; // A data vai vir como string do backend
}

@Injectable({
  providedIn: 'root'
})
export class EquipmentService {

  private apiUrl = 'http://localhost:5090/api/equipments';  // URL da API



  constructor(private http: HttpClient) { }

  getAll(): Observable<Equipment[]> {
    return this.http.get<Equipment[]>(this.apiUrl);
  }

  getById(id: number): Observable<Equipment> {
    return this.http.get<Equipment>(`${this.apiUrl}/${id}`);
  }
}
