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

  const placeholderImage =
    'https://media.istockphoto.com/id/1147544807/vector/thumbnail-image-vector-graphic.jpg?s=612x612&w=0&k=20&c=rnCKVbdxqkjlcs3xH87-9gocETqpspHFXu5dIGB4wuM=';

  if (!images.length) {
    return (
      <div className={`aspect-square ${className} flex`}>
        <img
          src={placeholderImage}
          alt={`${altText} - view 1`}
          className="w-full h-full object-cover"
        />
      </div>
    );
  }

  if (images.length === 1 || disabled === true) {
    return (
      <div className={`aspect-square ${className}`}>
        <img
          src={images[0]}
          alt={`${altText} - view 1`}
          className="w-full h-full object-cover"
        />
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
    <div className={`relative aspect-square ${className}`}>
      <div className="relative h-full w-full overflow-hidden">
        <div
          className="flex h-full transition-transform duration-300 ease-out"
          style={{ transform: `translateX(-${currentImage * 100}%)` }}
        >
          {images.map((url, index) => (
            <div key={`image-${index}`} className="flex-shrink-0 w-full h-full">
              <img
                src={url}
                alt={`${altText} - view ${index + 1}`}
                className="w-full h-full object-cover"
              />
            </div>
          ))}
        </div>
      </div>

      <button
        onClick={prevImage}
        className="absolute left-2 top-1/2 -translate-y-1/2 p-2 rounded-full bg-black/50 text-white hover:bg-black/75 transition-colors"
      >
        <ChevronLeft className="w-6 h-6" />
      </button>

      <button
        onClick={nextImage}
        className="absolute right-2 top-1/2 -translate-y-1/2 p-2 rounded-full bg-black/50 text-white hover:bg-black/75 transition-colors"
      >
        <ChevronRight className="w-6 h-6" />
      </button>

      {showCounter && images.length > 1 && (
        <div className="absolute bottom-4 right-4 bg-black/50 text-white px-3 py-1 rounded-full text-sm">
          {currentImage + 1} / {images.length}
        </div>
      )}
    </div>
  );
};

export default MushroomImageCarousel;
