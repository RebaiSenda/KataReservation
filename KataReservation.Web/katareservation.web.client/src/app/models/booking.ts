import { Person } from "./person";
import { Room } from "./room";

export interface Booking {
    id: number;
    roomId: number;
    personId: number;
    bookingDate: Date;
    startSlot: number;
    endSlot: number;
    room?: Room;
    person?: Person;
  }
  export interface CreateBookingRequest {
    roomId: number;
    personId: number;
    startTime: Date;
    endTime: Date;
  }