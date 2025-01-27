import { useState } from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';

interface MushroomImageCarouselProps {
  images: string[];
  altText?: string;
  className?: string;
  showCounter?: boolean;
  disabled?: boolean;
}

const MushroomImageCarousel = ({
  images,
  altText = 'Image showing a mushroom',
  className,
  disabled = false,
  showCounter = true
}: MushroomImageCarouselProps) => {
  const [currentImage, setCurrentImage] = useState(0);

  if (!images.length) return null;

  if (images.length === 1 || disabled === true) {
    return (
      <div
        className={`relative w-full h-auto max-w-[600px] ${className}`}
        style={{ aspectRatio: '16/9' }}
      >
        <div className="h-full w-full flex items-center justify-center overflow-hidden bg-black">
          <img
            src={images[0]}
            alt={`${altText} - view 1`}
            className="max-w-full max-h-full object-contain"
          />
        </div>
      </div>
    );
  }

  const nextImage = () => {
    setCurrentImage((prev) => (prev === images.length - 1 ? 0 : prev + 1));
  };

  const prevImage = () => {
    setCurrentImage((prev) => (prev === 0 ? images.length - 1 : prev - 1));
  };

  return (
    <div
      className={`relative w-full h-auto max-w-[700px] ${className}`}
      style={{ aspectRatio: '16/9' }}
    >
      <div className="relative h-full w-full flex items-center justify-center overflow-hidden rounded-lg bg-black">
        <div
          className="flex h-full transition-transform duration-300 ease-out"
          style={{ transform: `translateX(-${currentImage * 100}%)` }}
        >
          {images.map((url, index) => (
            <div
              key={`image-${index}`}
              className="flex-shrink-0 w-full h-full flex items-center justify-center"
            >
              <img
                src={url}
                alt={`${altText} - view ${index + 1}`}
                className="max-w-full max-h-full object-contain"
              />
            </div>
          ))}
        </div>
      </div>

      <button
        onClick={prevImage}
        className="absolute left-2 top-1/2 -translate-y-1/2 p-2 rounded-full bg-black/50 text-white hover:bg-black/75"
      >
        <ChevronLeft className="w-6 h-6" />
      </button>

      <button
        onClick={nextImage}
        className="absolute right-2 top-1/2 -translate-y-1/2 p-2 rounded-full bg-black/50 text-white hover:bg-black/75"
      >
        <ChevronRight className="w-6 h-6" />
      </button>

      {showCounter && (
        <div className="absolute bottom-4 right-4 bg-black/50 text-white px-2 py-1 rounded-md text-sm">
          {currentImage + 1} / {images.length}
        </div>
      )}
    </div>
  );
};

export default MushroomImageCarousel;
