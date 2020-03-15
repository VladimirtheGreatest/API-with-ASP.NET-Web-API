using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps/{moniker}/talks")]
    public class TalksController : ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public TalksController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
       [Route()]
        public async Task<IHttpActionResult> Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                var results = await _repository.GetTalksByMonikerAsync(moniker, includeSpeakers).ConfigureAwait(false);

                return Ok(_mapper.Map<IEnumerable<TalkModel>>(results));

            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
        [Route("{id:int}", Name = "GetTalk")]
        public async Task<IHttpActionResult> Get(string moniker, int id, bool includeSpeakers = false)
        {
            try
            {
                var result = await _repository.GetTalkByMonikerAsync(moniker, id, includeSpeakers).ConfigureAwait(false);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<TalkModel>(result));

            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
        [Route()]
        public async Task<IHttpActionResult> Post(string moniker, TalkModel model)
        {
            try
            {
              
                //validation
                if (ModelState.IsValid)
                {
                    var camp = await _repository.GetCampAsync(moniker).ConfigureAwait(false);
                    if (camp != null)
                    {
                        var talk = _mapper.Map<Talk>(model);
                        talk.Camp = camp;

                        //mapping the speaker if we need that
                        if (model.Speaker != null)
                        {
                            var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                            if (speaker != null) talk.Speaker = speaker;
                        }

                        _repository.AddTalk(talk);

                        if (await _repository.SaveChangesAsync().ConfigureAwait(false))
                        {
                            var newModel = _mapper.Map<TalkModel>(talk);
                            return CreatedAtRoute("GetTalk", new { moniker = moniker, talk = talk.TalkId}, newModel);
                        }
                    }                 
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);

            }

            return BadRequest(ModelState);

        }
        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Put(string moniker, int talkId, TalkModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var talk = await _repository.GetTalkByMonikerAsync(moniker, talkId, true).ConfigureAwait(false);
                    if (talk == null) return NotFound();

                    //take the talk model and put it into talk, ignoring speaker
                    _mapper.Map(model, talk);

                    //speaker separately 
                    if (talk.Speaker.SpeakerId != model.Speaker.SpeakerId)
                    {
                        var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId).ConfigureAwait(false);
                        if (speaker != null)
                        {
                            talk.Speaker = speaker;
                        }
                    }


                    if (await _repository.SaveChangesAsync())
                    {
                        return Ok(_mapper.Map<TalkModel>(model));
                    }
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        }

    }
}
