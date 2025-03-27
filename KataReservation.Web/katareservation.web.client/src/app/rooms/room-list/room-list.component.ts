import { Component, OnInit } from '@angular/core';
import { Room } from '../../models/room';
import { RoomService } from '../../services/room.service';
import { MatDialog } from '@angular/material/dialog';
import { RoomFormComponent } from '../room-form/room-form.component';

@Component({
  selector: 'app-room-list',
  templateUrl: './room-list.component.html',
  styleUrls: ['./room-list.component.scss']
})
export class RoomListComponent implements OnInit {
  rooms: Room[] = [];
  displayedColumns: string[] = ['id', 'roomName', 'actions'];

  constructor(
    private roomService: RoomService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.roomService.getRooms().subscribe(rooms => {
      console.log({rooms});
      this.rooms = rooms;
    });
  }

  openRoomForm(room?: Room): void {
    const dialogRef = this.dialog.open(RoomFormComponent, {
      width: '400px',
      data: room || {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadRooms();
      }
    });
  }

  deleteRoom(id: number): void {
    if (confirm('Are you sure you want to delete this room?')) {
      this.roomService.deleteRoom(id).subscribe(() => {
        this.loadRooms();
      });
    }
  }
}