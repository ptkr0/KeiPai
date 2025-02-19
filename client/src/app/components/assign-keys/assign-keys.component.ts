import { Component, CUSTOM_ELEMENTS_SCHEMA, forwardRef, Input, OnChanges, SimpleChanges } from '@angular/core';
import { IAssignKeys, IKeysPerPlatform } from '../../models/key.model';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { PlatformIconPipe } from "../../pipes/platform-icon.pipe";
import { bootstrapXbox, bootstrapPlaystation, bootstrapNintendoSwitch, bootstrapAndroid, bootstrapApple, bootstrapGlobe } from '@ng-icons/bootstrap-icons';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { simpleSteam, simpleEpicgames, simpleOrigin, simpleUbisoft, simpleBattledotnet, simpleGogdotcom, simpleItchdotio } from '@ng-icons/simple-icons';

@Component({
  selector: 'app-assign-keys',
  standalone: true,
  imports: [PlatformIconPipe, NgIconComponent, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AssignKeysComponent),
      multi: true,
    },
    provideIcons({
      simpleSteam,
      simpleEpicgames,
      simpleOrigin,
      simpleUbisoft,
      simpleBattledotnet,
      simpleGogdotcom,
      simpleItchdotio,
      bootstrapXbox,
      bootstrapPlaystation,
      bootstrapNintendoSwitch,
      bootstrapAndroid,
      bootstrapApple,
      bootstrapGlobe,
    }),
  ],
  templateUrl: './assign-keys.component.html',
  styleUrl: './assign-keys.component.css',
})
export class AssignKeysComponent implements ControlValueAccessor, OnChanges {
  @Input()
  keys: IKeysPerPlatform[] = [];

  selectedKeys: { [platformId: number]: IAssignKeys } = {};

  private onChange: (value: IAssignKeys[]) => void = () => { };
  private onTouched: () => void = () => { };

  writeValue(obj: IAssignKeys[]): void {
    if (obj) {
      this.selectedKeys = {};
      obj.forEach((assignKey) => {
        this.selectedKeys[assignKey.platformId] = assignKey;
      });
    } else {
      this.selectedKeys = {};
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['keys'] && !changes['keys'].firstChange) {
      this.selectedKeys = {};
    }
  }

  registerOnChange(fn: (value: IAssignKeys[]) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  trackByPlatformId(index: number, item: IKeysPerPlatform): number {
    return item.platformId;
  }

  getSelectedNumberOfKeys(platformId: number): number {
    const assignKey = this.selectedKeys[platformId];
    return assignKey ? assignKey.numberOfKeys : 0;
  }

  getIsUnlimited(platformId: number): boolean {
    const assignKey = this.selectedKeys[platformId];
    return assignKey ? assignKey.isUnlimited : false;
  }

  onSliderChange(event: Event, platformId: number): void {
    const inputElement = event.target as HTMLInputElement;
    const numberOfKeys = parseInt(inputElement.value, 10);

    if (!this.selectedKeys[platformId]) {
      this.selectedKeys[platformId] = {
        platformId,
        numberOfKeys,
        isUnlimited: false,
      };
    } else {
      this.selectedKeys[platformId].numberOfKeys = numberOfKeys;
    }

    if (this.selectedKeys[platformId].numberOfKeys === 0 && !this.selectedKeys[platformId].isUnlimited) {
      delete this.selectedKeys[platformId];
    }

    this.updateValue();
  }

  onCheckboxChange(event: Event, platformId: number): void {
    const inputElement = event.target as HTMLInputElement;
    const isChecked = inputElement.checked;

    if (!this.selectedKeys[platformId]) {
      this.selectedKeys[platformId] = {
        platformId,
        numberOfKeys: 0,
        isUnlimited: isChecked,
      };
    } else {
      this.selectedKeys[platformId].isUnlimited = isChecked;
    }

    if (this.selectedKeys[platformId].numberOfKeys === 0 && !this.selectedKeys[platformId].isUnlimited) {
      delete this.selectedKeys[platformId];
    }

    this.updateValue();
  }

  updateValue(): void {
    const output: IAssignKeys[] = Object.values(this.selectedKeys);
    this.onChange(output);
  }
}
