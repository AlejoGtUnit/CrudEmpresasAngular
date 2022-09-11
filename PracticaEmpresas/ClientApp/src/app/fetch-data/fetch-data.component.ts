import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from "@angular/common";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['./fetch-data.component.css']
})
export class FetchDataComponent {
  
  public empresas: ItemEmpresa[] = [];
  public baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
      this.http.get<ItemEmpresa[]>(this.baseUrl + 'empresas').subscribe(result => {
        console.log("Cantidad empresas obtenidas de Banco: " + result.length);
        this.empresas = result;
    }, error => console.error(error));
  }

  onClick(numero: string) {
    console.log("Eliminar empresa " + numero);
    if (confirm('¿Seguro que desea eliminar el registro?')) {
      this.http.delete(this.baseUrl + 'empresas/' + numero).subscribe(result => {
        alert("Se elimino correctamente!");
        this.ngOnInit();
      }, error => console.error(error));
    }
  }
}

export interface ItemEmpresa {
  numero: number;
  nombre: string;
  estado: string;
  email: string;
  nit: string;
}
