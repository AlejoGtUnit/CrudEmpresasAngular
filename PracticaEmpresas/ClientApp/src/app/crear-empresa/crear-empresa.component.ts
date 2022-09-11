import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { FormsModule, FormBuilder, FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { entradaEmpresa } from './entrada-empresa';

@Component({
  selector: 'app-crear-empresa',
  templateUrl: './crear-empresa.component.html',
  styleUrls: ['./crear-empresa.component.css']
})
export class CrearEmpresaComponent implements OnInit {

  public baseUrl: string;
  modeloEntrada = new entradaEmpresa('', '', '', '', '', '', '');
  mensajeError = '';
  mensajeExito = '';

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string)
  {
    this.baseUrl = baseUrl;
  }

  ngOnInit(): void {

  }

  onSubmit(): void {
    console.log(this.modeloEntrada);

    let todosLosCampos: boolean = true;
    this.mensajeError = "";
    this.mensajeExito = "";

    if (!this.modeloEntrada.nombreComercial || !this.modeloEntrada.razonSocial || !this.modeloEntrada.telefono || !this.modeloEntrada.correoElectronico || !this.modeloEntrada.nit || !this.modeloEntrada.estado || !this.modeloEntrada.direccion)
      todosLosCampos = false;

    if (!todosLosCampos) {
      this.mensajeError = "Debe ingresar todos los campos!";
      return;
    }

    this.http.post(this.baseUrl + "empresas", this.modeloEntrada)
      .subscribe(result => {
        console.log(result);
        if (result !== undefined) {
          this.mensajeExito = "Empresa creada exitosamente!";
          this.modeloEntrada = new entradaEmpresa('', '', '', '', '', '', '');
        }
        else {
          this.mensajeError = "No se pudo crear la empresa!";
        }
      },
      error => {
        console.error(error)
        this.mensajeError = "No se pudo crear la empresa!";
      });
  }
}
