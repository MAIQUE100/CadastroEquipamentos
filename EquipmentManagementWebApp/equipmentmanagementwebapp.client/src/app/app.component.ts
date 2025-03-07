import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { EquipmentService } from './Equipment/equipment.service';
import { Equipment } from './Equipment/equipment.service';  


//interface WeatherForecast {
//  date: string;
//  temperatureC: number;
//  temperatureF: number;
//  summary: string;
//}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit {
  title = 'Equipmentmanagementwebapp.client';
  equipmentList: Equipment[] = [];
  loading = true;
  errorMessage = '';

//export class AppComponent implements OnInit {
//  title = 'EquipmentManagement';
//  public equipmentList: Equipment[] = [];  // Lista de equipamentos
//  public loading = true;
//  public errorMessage = '';

  

  constructor(private http: HttpClient, private equipmentService: EquipmentService) { }

  

  ngOnInit() {
    this.loadEquipment();
  }

  //loadEquipment() {
  //  this.equipmentService.getAll().subscribe(
  //    (result) => {
  //      this.equipmentList = result;
  //      this.loading = false;
  //    },
  //    (error) => {
  //      console.error(error)
  //      this.loading = false;
  //    }
  //  )
  //}

  loadEquipment() {
    this.equipmentService.getAll().subscribe(
      (result) => {
        this.equipmentList = result;  // Salvar os equipamentos na lista
        this.loading = false;  // Parar o loading
      },
      (error) => {
        this.errorMessage = 'Erro ao carregar os equipamentos.';
        console.error(error);  // Logar o erro
        this.loading = false;
      }
    );
  }
}
