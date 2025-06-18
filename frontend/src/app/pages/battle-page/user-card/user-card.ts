import {Component, input, InputSignal} from '@angular/core';
import {
  HlmCardImports
} from '@spartan-ng/helm/card';
import {PlayerSummary} from '../../../../core/services/zod-types';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [
    HlmCardImports,
  ],
  templateUrl: './user-card.html',
  styleUrl: './user-card.css'
})
export class UserCard {
  public readonly user: InputSignal<PlayerSummary> = input.required();
}
