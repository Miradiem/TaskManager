import { Directive, ElementRef, Input, OnInit } from '@angular/core';

@Directive({
  selector: '[appAutoFocus]',
  standalone: true
})
export class AutoFocusDirective implements OnInit {
  @Input() appAutoFocus: boolean = true;
  @Input() delay: number = 0;

  constructor(private elementRef: ElementRef) {}

  ngOnInit(): void {
    if (this.appAutoFocus) {
      setTimeout(() => {
        this.elementRef.nativeElement.focus();
      }, this.delay);
    }
  }
}
