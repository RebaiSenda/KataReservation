export interface Room {
  id: number;
  roomName: string;
}

export interface CreateRoomRequest {
  roomName: string;
}

export interface UpdateRoomRequest {
  roomName: string;
}
