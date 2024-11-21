import { Input, Component } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

@Component({
  selector: 'pokemon',
  templateUrl: 'pokemon.component.html',
  styleUrl: 'pokemon.component.css',
  standalone: true,
  imports: [
    BrowserModule, HttpClientModule
  ],
})
export class PokemonComponent {
  @Input() name : string = "";
  @Input() id : number = 0;
  @Input() wins : number = 0;
  @Input() losses : number = 0;
  @Input() ties : number = 0;
  @Input() type : string = "";
}
