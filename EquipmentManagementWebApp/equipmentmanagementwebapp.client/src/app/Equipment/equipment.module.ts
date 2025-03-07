import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EquipmentListComponent } from './equipment-list/equipment-list.component';
import { EquipmentService } from './equipment.service';

@NgModule({
  declarations: [
    EquipmentListComponent
  ],
  imports: [
    CommonModule
  ],
  exports: [EquipmentListComponent],
  providers: [EquipmentService]
})
export class EquipmentModule { }
