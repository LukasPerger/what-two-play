import {Component, input, InputSignal} from '@angular/core';
import {HlmCardImports} from '@spartan-ng/helm/card';
import {HlmBadgeDirective} from '@spartan-ng/helm/badge';
import {NgOptimizedImage} from '@angular/common';
import {HlmSliderImports} from '@spartan-ng/helm/slider';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-game-card',
  standalone: true,
  imports: [
    HlmBadgeDirective,
    HlmCardImports,
    NgOptimizedImage,
    HlmSliderImports,
    FormsModule
  ],
  templateUrl: './game-card.html',
  styleUrl: './game-card.css'
})
export class GameCard {
  readonly gameCardData: InputSignal<GameCardData> = input.required();
}

export interface GameCardData {
  genres: string[];
  tags: string[];
  steamRating : number;
}
