import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import Room from "../models/room";

@Injectable({ providedIn: "root" })
export default class AppRoomService {
    constructor(private readonly http: HttpClient) { }

    add = (user: Room) => this.http.post<number>("api/rooms", user);

    delete = (id: number) => this.http.delete(`api/rooms/${id}`);

    get = (id: number) => this.http.get<Room>(`api/rooms/${id}`);

    inactivate = (id: number) => this.http.patch(`api/rooms/${id}/inactivate`, {});

    list = () => this.http.get<Room[]>("api/rooms");

    update = (r: Room) => this.http.put(`api/rooms/${r.Id}`, r);
}
