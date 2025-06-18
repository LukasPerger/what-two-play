import {Component, input, InputSignal} from '@angular/core';
import {HlmCardImports} from '@spartan-ng/helm/card';
import {HlmBadgeDirective} from '@spartan-ng/helm/badge';
import {NgOptimizedImage} from '@angular/common';
import {HlmSliderImports} from '@spartan-ng/helm/slider';
import {FormsModule} from '@angular/forms';
import {OwnedGame} from '../../../../core/services/zod-types';

@Component({
  selector: 'app-game-card',
  standalone: true,
  imports: [
    HlmBadgeDirective,
    HlmCardImports,
    NgOptimizedImage,
    FormsModule
  ],
  templateUrl: './game-card.html',
  styleUrl: './game-card.css'
})
export class GameCard {
  public readonly gameCardData: InputSignal<OwnedGame> = input.required();
}
