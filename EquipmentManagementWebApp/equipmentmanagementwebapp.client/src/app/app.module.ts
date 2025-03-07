import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { EquipmentModule } from './Equipment/equipment.module';
import { AppComponent } from './app.component';


@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    HttpClientModule,  // Importante para chamadas HTTP
    EquipmentModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
