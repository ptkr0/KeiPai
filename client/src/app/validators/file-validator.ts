// file-validator.ts
import { AbstractControl, ValidatorFn } from '@angular/forms';

export function fileValidator(): ValidatorFn {
  return (control: AbstractControl) => {
    const file = control.value;
    if (file) {
      const maxSizeInBytes = 1024 * 1024;
      const allowedTypes = ['image/jpeg', 'image/png', 'image/webp', 'image/jpg'];

      if (file.size > maxSizeInBytes) {
        return {
          maxSize: {
            actualSize: file.size,
            maxSize: maxSizeInBytes
          }
        };
      }

      if (!allowedTypes.includes(file.type)) {
        return {
          invalidType: true
        };
      }
    }

    return null;
  };
}
