export interface Person {
  id: number;
  firstName: string;
  lastName: string;
}

export interface CreatePersonRequest {
  firstName: string;
  lastName: string;
}

export interface UpdatePersonRequest {
  firstName: string;
  lastName: string;
}
