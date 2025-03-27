import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Person } from '../../models/person';
import { PersonService } from '../../services/person.service';

@Component({
  selector: 'app-person-form',
  templateUrl: './person-form.component.html',
  styleUrls: ['./person-form.component.scss']
})
export class PersonFormComponent implements OnInit {
  personForm: FormGroup;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private personService: PersonService,
    private dialogRef: MatDialogRef<PersonFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Person
  ) {
    this.personForm = this.fb.group({
      id: [0],
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]]
    });
  }

  ngOnInit(): void {
    if (this.data && this.data.id) {
      this.isEditMode = true;
      this.personForm.patchValue(this.data);
    }
  }

  onSubmit(): void {
    if (this.personForm.invalid) {
      return;
    }

    const person: Person = this.personForm.value;

    if (this.isEditMode) {
      this.personService.updatePerson(person).subscribe({
        next: () => this.dialogRef.close(true),
        error: (error) => console.error('Error updating person:', error)
      });
    } else {
      this.personService.createPerson(person).subscribe({
        next: () => this.dialogRef.close(true),
        error: (error) => console.error('Error creating person:', error)
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}