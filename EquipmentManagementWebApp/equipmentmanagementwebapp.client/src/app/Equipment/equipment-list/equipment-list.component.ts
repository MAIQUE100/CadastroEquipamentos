import { Component, OnInit } from '@angular/core';
import { EquipmentService, Equipment } from '../equipment.service';

@Component({
  selector: 'app-equipment-list',
  templateUrl: './equipment-list.component.html',
  styleUrls: ['./equipment-list.component.css'],
  standalone: false,
})
export class EquipmentListComponent implements OnInit {
  equipmentList: Equipment[] = [];
  loading = true;
  errorMessage = '';

  constructor(private equipmentService: EquipmentService) { }

  ngOnInit(): void {
    this.loadEquipment();
  }

  loadEquipment(): void {
    this.equipmentService.getAll().subscribe({
      next: (data) => {
        this.equipmentList = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao buscar equipamentos:', error);
        this.errorMessage = 'Erro ao carregar os equipamentos.';
        this.loading = false;
      }
    });
  }
}
