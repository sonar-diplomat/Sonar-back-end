using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
   public class MusicStreamResultDTO
    {
        private FileStream _stream;
        private string _contentType;
        private bool _supportsRange;

        public MusicStreamResultDTO(FileStream stream, string contentType, bool supportsRange)
        {
            _stream = stream;
            _contentType = contentType;
            _supportsRange = supportsRange;
        }

        public void GetStreamDetails(out Stream stream, out string contentType, out bool supportsRange)
        {
            stream = _stream;
            contentType = _contentType;
            supportsRange = _supportsRange;
        }
    }
}
