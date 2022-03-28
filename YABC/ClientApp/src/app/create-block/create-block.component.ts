import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { Person } from '../models/person';
import { CreateBlockViewModel } from '../models/create-block';

@Component({
  selector: 'app-create-block',
  templateUrl: './create-block.component.html',
  styleUrls: ['./create-block.component.scss']
})
export class CreateBlockComponent implements OnInit {

  name = new FormControl('');
  
  @Input("people") 
  public people!: Person[]; 

  @Output("submitAction")
  public submitAction = new EventEmitter<CreateBlockViewModel>();

  isSubmitted: boolean = false;

  constructor(public fb: FormBuilder,private cd: ChangeDetectorRef) {}

  createBlockForm = this.fb.group({
    name: ['', [Validators.required]],
    person: ['', [Validators.required]],
    image: ['', [Validators.required]]
  });

  ngOnInit(): void {
  }

  public get f() {
    return this.createBlockForm;
  }

  onSubmit(): boolean {
    this.isSubmitted=true;
    if (!this.createBlockForm.valid) {
      return false;
    } else {
      if (this.submitAction) {
        let createBlock: CreateBlockViewModel = {
          image: this.createBlockForm.controls['image'].value,
          name:  this.createBlockForm.controls['name'].value,
          personId: this.createBlockForm.controls['person'].value
        };
        this.submitAction.emit(createBlock);
      }
      return true;
    }
  }

  onFileChange(event: any) {
    let reader = new FileReader();
   
    if(event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);
    
      reader.onload = () => {
        this.createBlockForm.patchValue({
          image: reader.result
        });
        
         this.cd.markForCheck();
      };
    }
  }

}
