import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule, FormBuilder, FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { entradaEmpresa } from '../crear-empresa/entrada-empresa';
import { HttpClient } from '@angular/common/http';
import { ItemEmpresa } from '../fetch-data/fetch-data.component';

@Component({
  selector: 'app-editar-empresa',
  templateUrl: './editar-empresa.component.html',
  styleUrls: ['./editar-empresa.component.css']
})
export class EditarEmpresaComponent implements OnInit {

  public baseUrl: string;
  public numero?: string = "";
  modeloEntrada = new entradaEmpresa('', '', '', '', '', '', '');
  mensajeError = '';
  mensajeExito = '';

  constructor(private route: ActivatedRoute, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  ngOnInit(): void {
    this.numero = (this.route.snapshot.paramMap.get("numero"))?.toString();
    console.log('Numero de empresa:' + this.numero);

    this.http.get<empresa>(this.baseUrl + 'empresas/' + this.numero).subscribe(result => {

      console.log(result);
      this.modeloEntrada.correoElectronico = result.correo;
      this.modeloEntrada.direccion = result.direccion;
      this.modeloEntrada.estado = result.estado;
      this.modeloEntrada.nit = result.nit;
      this.modeloEntrada.nombreComercial = result.nombre_comercial;
      this.modeloEntrada.razonSocial = result.razon_social;
      this.modeloEntrada.telefono = result.telefono;

    }, error => console.error(error));
  }

  onSubmit(): void {
    console.log(this.modeloEntrada);

    let todosLosCampos: boolean = true;
    this.mensajeError = "";
    this.mensajeExito = "";

    if (!this.modeloEntrada.nombreComercial || !this.modeloEntrada.razonSocial || !this.modeloEntrada.telefono || !this.modeloEntrada.correoElectronico || !this.modeloEntrada.nit || !this.modeloEntrada.estado || !this.modeloEntrada.direccion)
      todosLosCampos = false;

    if (!todosLosCampos) {
      this.mensajeError = "Todos los campos deben ser ingresados!";
      return;
    }

    this.http.put(this.baseUrl + "empresas/" + this.numero, this.modeloEntrada)
      .subscribe(result => {
        console.log(result);
        if (result !== undefined) {
          this.mensajeExito = "Empresa modificada exitosamente!";
        }
        else {
          this.mensajeError = "No se pudo modificar la empresa!";
        }
      },
        error => {
          console.error(error)
          this.mensajeError = "No se pudo modificar la empresa!";
        });
  }
}

interface empresa {
  id: number;
  nombre_comercial: string;
  razon_social: string;
  telefono: string;
  correo: string;
  nit: string;
  direccion: string;
  estado: string;
  created_at: string;
  updated_at: string;
}
