namespace tlogThumbnailHandler
{
    enum IEIFLAG
    {
        ASYNC = 0x0001, // ask the extractor if it supports ASYNC extract (free threaded)      
        CACHE = 0x0002, // returned from the extractor if it does NOT cache the thumbnail      
        ASPECT = 0x0004, // passed to the extractor to beg it to render to the aspect ratio of the supplied rect       
        OFFLINE = 0x0008, // if the extractor shouldn't hit the net to get any content neede for the rendering     
        GLEAM = 0x0010, // does the image have a gleam ? this will be returned if it does       
        SCREEN = 0x0020, // render as if for the screen (this is exlusive with IEIFLAG_ASPECT )      
        ORIGSIZE = 0x0040, // render to the approx size passed, but crop if neccessary       
        NOSTAMP = 0x0080, // returned from the extractor if it does NOT want an icon stamp on the thumbnail    
        NOBORDER = 0x0100, // returned from the extractor if it does NOT want an a border around the thumbnail    
        QUALITY = 0x0200 // passed to the Extract method to indicate that a slower, higher quality image is desired, re-compute the thumbnail    
    }
}