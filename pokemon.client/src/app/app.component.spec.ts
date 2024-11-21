import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { Pokemon } from './data/pokemon'

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [HttpClientTestingModule]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should retrieve pokemon from the server', () => {
    const mockPokemon : Pokemon[] = [
      { name: 'test mon', wins: 3, losses:4, ties: 0, id:0, types:[{type:{name:"normal"}}]},
      { name: 'test mon', wins: 3, losses:4, ties: 0, id:0, types:[{type:{name:"normal"}}]}
    ];

    component.ngOnInit();
    component.getPokemon("name", "asc");

    const req = httpMock.expectOne('/pokemon/tournament/statistics?orderby=name&sortDirection=asc');
    expect(req.request.method).toEqual('GET');
    req.flush(mockPokemon);

    expect(component.pokemon).toEqual(mockPokemon);
  });

  it('should handle errors', () => {
    const mockPokemon : Pokemon[] = [
      { name: 'test mon', wins: 3, losses:4, ties: 0, id:0, types:[{type:{name:"normal"}}]},
      { name: 'test mon', wins: 3, losses:4, ties: 0, id:0, types:[{type:{name:"normal"}}]}
    ];

    component.ngOnInit();
    component.getPokemon("name", "nogood");

    const req = httpMock.expectOne('/pokemon/tournament/statistics?orderby=name&sortDirection=asc');
    expect(req.request.method).toEqual('GET');
    req.flush(mockPokemon);

    expect(component.errorMessage).toBeTruthy();
  });
});