export interface Pokemon {
  name: string;
  wins: number;
  losses: number;
  ties: number;
  id: number;
  types: [
    {
      type: {
        name: string;
      }
    }
  ]
}