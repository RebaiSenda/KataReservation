export interface Room {
    id: number;
    roomName: string;
  }
  export interface CreateRoomRequest {
    name: string;
    capacity: number;
  }