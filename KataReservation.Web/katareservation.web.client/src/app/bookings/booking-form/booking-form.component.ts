import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { Booking } from '../../models/booking';
import { Room } from '../../models/room';
import { Person } from '../../models/person';
import { BookingService } from '../../services/booking.service';
import { RoomService } from '../../services/room.service';
import { PersonService } from '../../services/person.service';

@Component({
  selector: 'app-booking-form',
  templateUrl: './booking-form.component.html',
  styleUrls: ['./booking-form.component.scss']
})
export class BookingFormComponent implements OnInit {
  bookingForm: FormGroup;
  rooms: Room[] = [];
  persons: Person[] = [];
  timeSlots: number[] = [8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];

  constructor(
    private fb: FormBuilder,
    private bookingService: BookingService,
    private roomService: RoomService,
    private personService: PersonService,
    private dialogRef: MatDialogRef<BookingFormComponent>
  ) {
    this.bookingForm = this.fb.group({
      roomId: ['', Validators.required],
      personId: ['', Validators.required],
      bookingDate: ['', Validators.required],
      startSlot: ['', Validators.required],
      endSlot: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadRooms();
    this.loadPersons();
    
    // Set default values
    this.bookingForm.patchValue({
      bookingDate: new Date(),
      startSlot: 9,
      endSlot: 10
    });
  }

  loadRooms(): void {
    this.roomService.getRooms().subscribe(rooms => {
      this.rooms = rooms;
    });
  }

  loadPersons(): void {
    this.personService.getPersons().subscribe(persons => {
      this.persons = persons;
    });
  }

  onSubmit(): void {
    if (this.bookingForm.invalid) {
      return;
    }

    const booking: Booking = {
      id: 0,
      ...this.bookingForm.value
    };

    this.bookingService.createBooking(booking).subscribe({
      next: () => this.dialogRef.close(true),
      error: (error) => console.error('Error creating booking:', error)
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}