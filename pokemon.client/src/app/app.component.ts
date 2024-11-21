import { HttpClient, HttpParams  } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Pokemon } from "./data/pokemon"

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  public pokemon: Pokemon[] = [];

  errorMessage: any = "";
  generating = false;
  orderByOptions = ["name", "wins", "losses", "ties", "id"];
  sortDirectionOptions = ["desc", "asc"];

  orderBy = this.orderByOptions[0];
  sortDirection = this.sortDirectionOptions[0];

  constructor(private http: HttpClient) {}

  ngOnInit() {

  }

  getPokemon(orderBy : string, sortDirection : string) {
    let requestUrl = '/pokemon/tournament/statistics?orderBy=' + orderBy + "&sortDirection=" + sortDirection;
    this.generating = true;
    this.http.get<Pokemon[]>(requestUrl).subscribe(
      (result) => {
        this.errorMessage = "";
        this.pokemon = result;
        console.log(result);
        this.generating = false;
      },
      (error) => {
        this.errorMessage = error.error;
        console.error(error);
        console.error(error.error);
        this.generating = false;
      }
    );
  }

  onOrderByChanged(event : any) {
    this.orderBy = event.target.value;
  }

  onSortDirectionChanged(event : any) {
    this.sortDirection = event.target.value;
  }

  generateNewStatistics() {
    this.getPokemon(this.orderBy, this.sortDirection);
  }

  testErrorOne() {
    this.getPokemon(this.orderBy, "nogood");
  }

  testErrorTwo() {
    this.getPokemon("", "asc");
  }

  testErrorThree() {
    this.getPokemon("beepboop", "asc");
  }

  title = 'pokemon.client';
}
