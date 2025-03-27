import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Room } from '../../models/room';
import { RoomService } from '../../services/room.service';

@Component({
  selector: 'app-room-form',
  templateUrl: './room-form.component.html',
  styleUrls: ['./room-form.component.scss']
})
export class RoomFormComponent implements OnInit {
  roomForm: FormGroup;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private roomService: RoomService,
    private dialogRef: MatDialogRef<RoomFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Room
  ) {
    this.roomForm = this.fb.group({
      id: [0],
      roomName: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    if (this.data && this.data.id) {
      this.isEditMode = true;
      this.roomForm.patchValue(this.data);
    }
  }

  onSubmit(): void {
    if (this.roomForm.invalid) {
      return;
    }

    const room: Room = this.roomForm.value;

    if (this.isEditMode) {
      this.roomService.updateRoom(room).subscribe({
        next: () => this.dialogRef.close(true),
        error: (error) => console.error('Error updating room:', error)
      });
    } else {
      this.roomService.createRoom(room).subscribe({
        next: () => this.dialogRef.close(true),
        error: (error) => console.error('Error creating room:', error)
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}